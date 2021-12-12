import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LineItemService } from '../../_services/lineItem.service';

@Component({
	selector: 'app-show-line-item',
	templateUrl: './show-line-item.component.html',
	styleUrls: ['./show-line-item.component.scss']
})
export class ShowLineItemComponent implements OnInit
{
	public id: string;
	public lineItemData: any;
	public lineItemForm: FormGroup = new FormGroup({});
	public editMode: boolean = false;

	constructor(private lineItemService: LineItemService, private route: ActivatedRoute, private router: Router)
	{
	}

	ngOnInit(): void
	{
		this.id = this.route.snapshot.paramMap.get('id');
		if (this.id == null)
		{
			this.editMode = true;
		}
		this.lineItemService.get(this.id)
			.subscribe((data: any) =>
			{
				this.lineItemData = data.fields;
				for (var i = 0; i < this.lineItemData.length; i++)
				{
					if (this.id == null && this.lineItemData[i].value == 0)
					{
						this.lineItemData[i].value = "";
					}
					this.lineItemForm.addControl(this.lineItemData[i].name, new FormControl(this.lineItemData[i].value));
				}
			}, error =>
			{
				// TODO: handle errors
			});
	}

	setEditMode()
	{
		this.editMode = true;
	}

	cancel()
	{
		this.editMode = false;
	}

	save()
	{
		var fields: any[] = [];
		for (const [key, value] of Object.entries(this.lineItemForm.value))
		{
			fields.push({
				name: key,
				value: value.toString()
			})
		}

		var obj = {
			id: this.id,
			fields: fields
		};

		this.lineItemService.save(obj)
			.subscribe(data =>
			{
				this.editMode = false;
				this.router.navigate(['/LineItems/Show/' + data]);
			}, error =>
			{
				// TODO: handle errors
			});
	}
}
