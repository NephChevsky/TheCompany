import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { LineItemService } from '../../_services/lineItem.service';

@Component({
	selector: 'app-create-line-item',
	templateUrl: './create-line-item.component.html',
	styleUrls: ['./create-line-item.component.scss']
})
export class CreateLineItemComponent implements OnInit
{
	public lineItemForm: FormGroup = new FormGroup({});
	public lineItemData: any[];

	constructor(private lineItemService: LineItemService, private router: Router) { }

	ngOnInit(): void
	{
		this.lineItemService.getFields()
			.subscribe(
				data =>
				{
					this.lineItemData = data.fields;
					for (var i = 0; i < this.lineItemData.length; i++)
					{
						this.lineItemForm.addControl(this.lineItemData[i].name, new FormControl(""));
					}
				},
				error =>
				{
					/* TODO: set errors on field */
				});
	}

	onCancel()
	{
		this.router.navigate(['/LineItems']);
	}

	onSubmit()
	{
		var fields: any[] = [];
		for (var i = 0; i < this.lineItemData.length; i++)
		{
			var field = {
				name: this.lineItemData[i].name,
				value: this.lineItemForm.get(this.lineItemData[i].name).value.toString()
			}
			fields.push(field);
		}

		var obj = {
			fields: fields
		}

		this.lineItemService.create(obj)
			.subscribe(
				data =>
				{
					this.router.navigate(['/LineItems']);
				},
				error =>
				{
					/* TODO: set errors on field */
				});
	}
}
