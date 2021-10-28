import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdditionalFieldService } from 'src/app/_services/additionalField.service';
import { InvoiceService } from '../../_services/invoice.service';

@Component({
	selector: 'app-invoice-extraction',
	templateUrl: './invoiceextraction.component.html',
	styleUrls: ['./invoiceextraction.component.scss']
})
export class InvoiceExtractionComponent implements OnInit {

	fieldsName: string[] = ["InvoiceNumber", "CustomerId", "LastName", "FirstName", "Address"];
	invoiceSettingsForm: FormGroup = new FormGroup({});

	constructor(private formBuilder: FormBuilder,
				private router: Router,
				private invoiceService: InvoiceService,
				private additionalFieldService: AdditionalFieldService) { }

	ngOnInit(): void {
		this.invoiceSettingsForm = this.formBuilder.group({
			"fields": new FormArray([])
		});

		for (let item of this.fieldsName)
		{
			this.addField(item);
		}

		this.invoiceService.getExtractionSettings(this.fieldsName).subscribe((data: any[]) =>
		{
			for (var i = 0; i < data.length; i++)
			{
				this.updateField(data[i].field, '', data[i].x, data[i].y, data[i].height, data[i].width);
			}
		}, error => {
			// TODO
		});

		this.additionalFieldService.getFields("Invoice").subscribe((result: any) =>
		{
			var ids = []
			for (let item of result)
			{
				ids.push(item.id);
				this.addField(item.name, item.id);
			}
			this.invoiceService.getExtractionSettings(ids).subscribe((data: any) =>{
				for (var i = 0; i < data.length; i++)
				{
					this.updateField(data[i].field, data[i].id, data[i].x, data[i].y, data[i].height, data[i].width);
				}
			});
		}, error => {
			// TODO
		});
	}

	addField(name: string, id: string = "")
	{
		const tmpForm = this.formBuilder.group({
			name: [name, []],
			id: [id, []],
			x: ['', []],
			y: ['', []],
			height: ['',[]],
			width: ['',[]]
		});
		this.fields.push(tmpForm);
	}

	updateField(name: string, id = "", x = "", y = "", height = "", width = "")
	{
		for (let item of this.fields.controls)
		{
			if (item.get("name").value == name || item.get("id").value == name)
			{
				item.get('x').setValue(x);
				item.get('y').setValue(y);
				item.get('height').setValue(height);
				item.get('width').setValue(width);
				break;
			}
		}
	}

	get fields()
	{
		return this.invoiceSettingsForm.controls["fields"] as FormArray;
	}

	onSubmit()
	{
		if (this.invoiceSettingsForm.invalid)
		{
			return;
		}
		this.invoiceService.saveExtractionSettings(this.invoiceSettingsForm.value)
			.subscribe(
				data =>
				{
					this.router.navigate(['Home']);
					return;
				},
				error =>
				{
					/* TODO: set errors on field */
				});
	}

	onCancel()
	{
		this.router.navigate(['Home']);
	}
}
