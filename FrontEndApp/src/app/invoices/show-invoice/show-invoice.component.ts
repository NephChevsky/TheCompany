import { Component, OnInit, SecurityContext } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Filter } from 'src/app/_models/filter';
import { InvoiceService } from 'src/app/_services/invoice.service';

@Component({
	selector: 'app-show-invoice',
	templateUrl: './show-invoice.component.html',
	styleUrls: ['./show-invoice.component.scss']
})
export class ShowInvoiceComponent implements OnInit
{

	public id: string = "";
	public invoiceData: any[];
	public page: number = 1;
	public filters: Filter[] = [];
	public editMode: boolean = false;
	public invoiceForm: FormGroup = new FormGroup({});
	public lineItemsForm: FormGroup = new FormGroup({});
	public focusedFieldId: string;

	constructor(private invoiceService: InvoiceService,
		private route: ActivatedRoute,
		private formBuilder: FormBuilder,
		private router: Router)
	{
	}

	ngOnInit(): void
	{
		this.id = this.route.snapshot.paramMap.get('id');
		if (this.id)
		{
			this.filters.push(new Filter("InvoiceId", "=", this.id));
		}
		else
		{
			this.filters.push(new Filter("InvoiceId", "=", "00000000-0000-0000-0000-000000000000"));
			this.editMode = true;
		}

		this.invoiceService.getInvoice(this.id)
			.subscribe((data: any) =>
			{
				this.invoiceData = data.fields;
				for (var i = 0; i < this.invoiceData.length; i++)
				{
					this.invoiceForm.addControl(this.invoiceData[i].name, new FormControl(this.invoiceData[i].value));
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
		for (const [key, value] of Object.entries(this.invoiceForm.value))
		{
			fields.push({
				name: key,
				value: value
			})
		}

		var lineItems: any[] = [];
		var tmp = this.lineItemsForm.get("values") as FormArray;
		for (var i = 0; i < tmp.value.length; i++)
		{
			var lineItemsFields = [];
			var id;
			for (const [key, value] of Object.entries(tmp.at(i).value))
			{
				if (key == "Id")
				{
					id = value as string;
				}
				else
				{
					lineItemsFields.push({
						name: key,
						value: value.toString()
					});
				}
			}
			var item = {
				id: id,
				fields: lineItemsFields
			}
			lineItems.push(item);
		}

		var obj = {
			id: this.id,
			fields: fields,
			lineItems: lineItems
		};

		this.invoiceService.saveInvoice(obj)
			.subscribe(data =>
			{
				this.router.navigate(['/Invoices/Show/' + data]);
			}, error =>
			{
				// TODO: handle errors
			});
	}

	cancel()
	{
		this.editMode = false;
	}

	onFocus(target: EventTarget = null)
	{
		if (this.editMode)
		{
			var id = (target as HTMLInputElement).id;
			this.focusedFieldId = id;
		}
	}

	fillExtractedText(text: string)
	{
		if (this.editMode)
		{
			var input = document.getElementById(this.focusedFieldId) as HTMLInputElement;
			var tmp = this.focusedFieldId.split(".");
			if (tmp.length == 2)
				this.lineItemsForm.controls["values"].get([tmp[0]]).patchValue({ [tmp[1]]: text });
			else
				this.invoiceForm.patchValue({ [this.focusedFieldId]: text });

			input.focus();
		}
	}
}
