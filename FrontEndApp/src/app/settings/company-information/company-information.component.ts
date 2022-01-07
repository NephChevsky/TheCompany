import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Field } from '../../_models/field';
import { CompanyService } from '../../_services/company.service';

@Component({
	selector: 'app-company-information',
	templateUrl: './company-information.component.html',
	styleUrls: ['./company-information.component.scss']
})
export class CompanyInformationComponent implements OnInit
{
	public companyData: Field[];
	public companyForm: FormGroup = new FormGroup({});

	constructor(private companyService: CompanyService,
		private router: Router)
	{
	}

	ngOnInit(): void
	{
		this.companyService.get()
			.subscribe((data: any) =>
			{
				this.companyData = data.fields;
				for (var i = 0; i < this.companyData.length; i++)
				{
					this.companyForm.addControl(this.companyData[i].name, new FormControl(this.companyData[i].value));
				}
			}, error =>
			{
				// TODO: handle errors
			});
	}

	save()
	{
		var fields: any[] = [];
		for (const [key, value] of Object.entries(this.companyForm.value))
		{
			if (key != "Logo")
			{
				fields.push({
					name: key,
					value: value
				})
			}
		}

		let logo = new FormData();
		logo.append('file', this.companyForm.get("Logo").value);

		var obj = {
			fields: fields
		};
		
		this.companyService.save(obj)
			.subscribe(data =>
			{
				if (typeof (this.companyForm.get("Logo").value) != "string")
				{
					this.companyService.saveLogo(logo)
						.subscribe(data =>
						{
							this.router.navigate(['/Home']);
						}, error =>
						{
							// TODO: handle errors
						});
				}
				else
				{
					this.router.navigate(['/Home']);
				}
			}, error =>
			{
				// TODO: handle errors
			});
	}

	onCancel()
	{
		this.router.navigate(['/Home']);
	}
}
