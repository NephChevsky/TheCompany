import { HttpEventType } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Rectangle } from 'src/app/_models/rectangle';
import { PreviewComponent } from 'src/app/_modules/preview/preview.component';
import { AdditionalFieldService } from 'src/app/_services/additionalField.service';
import { InvoiceService } from '../../_services/invoice.service';

@Component({
	selector: 'app-invoice-extraction',
	templateUrl: './invoiceextraction.component.html',
	styleUrls: ['./invoiceextraction.component.scss']
})
export class InvoiceExtractionComponent implements OnInit
{
	lineItemsName: string[] = ["LineItem", "Reference", "Description", "Quantity", "UnitaryPrice", "Price"];
	invoiceSettingsForm: FormGroup = new FormGroup({});
	lineItemSettingsForm: FormGroup = new FormGroup({});
	error: string = "";
	progress: number = 0;
	inProgress: boolean = false;
	sampleFile: FormData = null;
	@ViewChild('appPreview', { static: false }) appPreview: PreviewComponent;


	constructor(private formBuilder: FormBuilder,
		private router: Router,
		private invoiceService: InvoiceService,
		private additionalFieldService: AdditionalFieldService) { }

	ngOnInit(): void
	{
		this.invoiceSettingsForm = this.formBuilder.group({
			"fields": new FormArray([])
		});

		this.lineItemSettingsForm = this.formBuilder.group({
			boxymin: ['', []],
			boxymax: ['', []],
			referencexmin: ['', []],
			referencexmax: ['', []],
			descriptionxmin: ['', []],
			descriptionxmax: ['', []],
			quantityxmin: ['', []],
			quantityxmax: ['', []],
			unitarypricexmin: ['', []],
			unitarypricexmax: ['', []],
			pricexmin: ['', []],
			pricexmax: ['', []]
		});

		this.invoiceService.getExtractionSettings().subscribe((data: any[]) =>
		{
			for (var i = 0; i < data.length; i++)
			{
				if (data[i].isLineItem)
				{
					if (data[i].field == "LineItem")
					{
						if (data[i].y != null)
						{
							this.lineItemSettingsForm.get("boxymin").setValue(data[i].y);
							if (data[i].height != null)
							{
								this.lineItemSettingsForm.get("boxymax").setValue(data[i].y + data[i].height);
							}
						}

					}
					else
					{
						if (data[i].x != null)
						{
							this.lineItemSettingsForm.get(data[i].field.toLowerCase() + "xmin").setValue(data[i].x);
							if (data[i].width != null)
							{
								this.lineItemSettingsForm.get(data[i].field.toLowerCase() + "xmax").setValue(data[i].x + data[i].width);
							}
						}
					}
				}
				else
				{
					this.addField(data[i].field);
					this.updateField(data[i].field, '', data[i].x, data[i].y, data[i].height, data[i].width);
				}

			}
		}, error =>
		{
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
			this.invoiceService.getExtractionSettings(ids).subscribe((data: any) =>
			{
				for (var i = 0; i < data.length; i++)
				{
					this.updateField(data[i].field, data[i].id, data[i].x, data[i].y, data[i].height, data[i].width);
				}
			});
		}, error =>
		{
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
			height: ['', []],
			width: ['', []]
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
		if (this.invoiceSettingsForm.invalid || this.lineItemSettingsForm.invalid)
		{
			return;
		}

		for (let control of (this.invoiceSettingsForm.get("fields") as FormArray).controls)
		{
			if (control.get("x").value == "" && control.get("y").value == "" && control.get("width").value == "" && control.get("height").value == "")
			{
				control.get("x").setValue(-1);
				control.get("y").setValue(-1);
				control.get("width").setValue(-1);
				control.get("height").setValue(-1);
			}
		}

		this.invoiceService.saveExtractionSettings(this.invoiceSettingsForm.value, this.lineItemSettingsForm.value)
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

	onFileChange(event: any)
	{
		if (event.target.files.length > 0)
		{
			this.sampleFile = new FormData();
			this.sampleFile.append("file", event.target.files[0]);
		}
		else
		{
			this.sampleFile = null;
		}
	}

	showBoundingBox(index: any)
	{
		if (this.sampleFile)
		{
			var rect;
			if (typeof (index) == "string")
			{
				if (index == "LineItemBox")
				{
					var x = 0;
					var y = parseInt((document.getElementById("boxymin") as HTMLInputElement).value);
					var width = 5000;
					var height = parseInt((document.getElementById("boxymax") as HTMLInputElement).value) - parseInt((document.getElementById("boxymin") as HTMLInputElement).value);
					rect = new Rectangle(x, y, width, height);
				}
				else
				{
					var x = parseInt((document.getElementById(index + 'xmin') as HTMLInputElement).value);
					var y = parseInt((document.getElementById("boxymin") as HTMLInputElement).value);
					var width = parseInt((document.getElementById(index + 'xmax') as HTMLInputElement).value) - parseInt((document.getElementById(index + 'xmin') as HTMLInputElement).value);
					var height = parseInt((document.getElementById("boxymax") as HTMLInputElement).value) - parseInt((document.getElementById("boxymin") as HTMLInputElement).value);
					rect = new Rectangle(x, y, width, height);
				}
			}
			else
			{
				var x = parseInt((document.getElementById('x' + index) as HTMLInputElement).value);
				var y = parseInt((document.getElementById('y' + index) as HTMLInputElement).value);
				var width = parseInt((document.getElementById('width' + index) as HTMLInputElement).value);
				var height = parseInt((document.getElementById('height' + index) as HTMLInputElement).value);
				rect = new Rectangle(x, y, width, height);
			}
			this.appPreview.drawBox(this.appPreview.reversePosition(rect));
		}
	}
}
