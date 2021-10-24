import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { InvoiceService } from '../_services/invoice.service';

@Component({
	selector: 'app-settings',
	templateUrl: './settings.component.html',
	styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

	invoiceSettingsForm: FormGroup = new FormGroup({});
	fieldSettingsForm: FormGroup = new FormGroup({});

	constructor(private formBuilder: FormBuilder,
				private router: Router,
				private invoiceService: InvoiceService) { }

	ngOnInit(): void {
		this.fieldSettingsForm = this.formBuilder.group({
			x: ['', [Validators.required]],
			y: ['', [Validators.required]],
			width: ['', [Validators.required]],
			height: ['', [Validators.required]]
		});
		
		this.invoiceSettingsForm = this.formBuilder.group({
			"invoiceNumber": this.fieldSettingsForm
		});

		this.invoiceService.getExtractionSettings().subscribe((data) =>{
			for (var i = 0; i < data.length; i++)
			{
				if (data[i].field == "InvoiceNumber")
				{
					this.invoiceSettingsForm.get('invoiceNumber')?.get('x')?.setValue(data[i].x);
					this.invoiceSettingsForm.get('invoiceNumber')?.get('y')?.setValue(data[i].y);
					this.invoiceSettingsForm.get('invoiceNumber')?.get('height')?.setValue(data[i].height);
					this.invoiceSettingsForm.get('invoiceNumber')?.get('width')?.setValue(data[i].width);
				}
			}
		})
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
