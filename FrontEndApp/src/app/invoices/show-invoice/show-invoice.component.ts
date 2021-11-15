import { Component, OnInit, SecurityContext } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Filter } from 'src/app/_models/filter';
import { InvoiceService } from 'src/app/_services/invoice.service';

@Component({
  selector: 'app-show-invoice',
  templateUrl: './show-invoice.component.html',
  styleUrls: ['./show-invoice.component.scss']
})
export class ShowInvoiceComponent implements OnInit {

	public id: string = "";
	public invoiceData: any;
	public page: number = 1;
	public filters: Filter[] = [];
	public editMode: boolean = false;
	public invoiceForm: FormGroup = new FormGroup({});
	public lineItemsForm: FormGroup = new FormGroup({});
	public focusedFieldId: string;

	constructor(private invoiceService: InvoiceService,
				private route: ActivatedRoute,
				private formBuilder: FormBuilder)
	{	
	}

	ngOnInit(): void
	{
		this.id = this.route.snapshot.paramMap.get('id');
		this.filters.push(new Filter("InvoiceId", "=", this.id));
		if (this.id)
		{
			this.invoiceService.getInvoice(this.id)
				.subscribe(data =>{
					this.invoiceData = data;
					for (var field in this.invoiceData)
					{
						this.invoiceForm.addControl(field, new FormControl(this.invoiceData[field], Validators.required));
					}
				}, error =>
				{
					// TODO: handle errors
				});
		}
	}

	setEditMode()
	{
		this.editMode = true;
	}

	save()
	{
		var fields = [];
		for (const [key, value] of Object.entries(this.invoiceForm.value))
		{
			fields.push({
				name: key,
				value: value
			})
		}
		var lineItems = [];

		var tmp = this.lineItemsForm.get("values") as FormArray;
		for (var i = 0; i < tmp.value.length; i++)
		{
			var lineItemsFields = [];
			var id;
			for (const [key, value] of Object.entries(tmp.at(i).value))
			{
				if (key == "Id")
					id = value as string;
				else
					lineItemsFields.push({
						name: key,
						value: value
					});
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
			.subscribe(data =>{
				window.location.reload()
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
				this.lineItemsForm.controls["values"].get([tmp[0]]).patchValue({[tmp[1]]:text});
			else
				this.invoiceForm.patchValue({[this.focusedFieldId]:text});
			
			input.focus();
		}
	}
}
