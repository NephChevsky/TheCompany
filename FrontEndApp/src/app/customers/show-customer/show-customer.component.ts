import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Filter } from '../../_models/filter';
import { CustomerEntityService } from '../../_services/customerEntity.service';

@Component({
	selector: 'app-show-customer',
	templateUrl: './show-customer.component.html',
	styleUrls: ['./show-customer.component.scss']
})
export class ShowCustomerComponent implements OnInit
{
	public id: string = "";
	public customerData: any[];
	public editMode: boolean = false;
	public customerForm: FormGroup = new FormGroup({});

	constructor(private route: ActivatedRoute,
		private customerService: CustomerEntityService,
		private router: Router) { }

	ngOnInit(): void
	{
		this.id = this.route.snapshot.paramMap.get('id');
		if (!this.id)
		{
			this.editMode = true;
		}

		this.customerService.get(this.id)
			.subscribe((data: any) =>
			{
				this.customerData = data.fields;
				for (var i = 0; i < this.customerData.length; i++)
				{
					this.customerForm.addControl(this.customerData[i].name, new FormControl(this.customerData[i].value));
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

	save()
	{
		var fields: any[] = [];
		for (const [key, value] of Object.entries(this.customerForm.value))
		{
			var field = this.customerData.find(x => x.name == key);
			if (field.editable)
			{
				fields.push({
					name: key,
					value: value
				});
			}
		}

		var obj = {
			id: this.id,
			fields: fields
		};

		this.customerService.save(obj)
			.subscribe(data =>
			{
				this.editMode = false;
				this.router.navigate(['/Customers/Show/' + data]);
			}, error =>
			{
				// TODO: handle errors
			});
	}

	cancel()
	{
		this.editMode = false;
	}
}
