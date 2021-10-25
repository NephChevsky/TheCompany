import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { InvoiceService } from '../_services/invoice.service';

@Component({
	selector: 'app-settings',
	templateUrl: './settings.component.html',
	styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

	fieldsName: string[] = ["InvoiceNumber", "CustomerId", "Address"];
	invoiceSettingsForm: FormGroup = new FormGroup({});

	constructor(private formBuilder: FormBuilder,
				private router: Router,
				private invoiceService: InvoiceService) { }

	ngOnInit(): void {		
		this.invoiceSettingsForm = this.formBuilder.group({
			"fields": new FormArray([])
		});

		for (let item of this.fieldsName)
		{
			const tmpForm = this.formBuilder.group({
				name: item,
				x: ['', []],
				y: ['', []],
				height: ['',[]],
				width: ['',[]]
			});
			this.fields.push(tmpForm);
		}

		this.invoiceService.getExtractionSettings().subscribe((data) =>{
			for (var i = 0; i < data.length; i++)
			{
				for (let item of this.fields.controls)
				{
					if (data[i].field == item.get("name")?.value)
					{
						item.get('x')?.setValue(data[i].x);
						item.get('y')?.setValue(data[i].y);
						item.get('height')?.setValue(data[i].height);
						item.get('width')?.setValue(data[i].width);
						break;
					}
				}
			}
		})
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
					this.router.navigate(['home']);
				},
				error =>
				{
					/* TODO: set errors on field */
				});
	}

	onCancel()
	{
		this.router.navigate(['home']);
	}
}
