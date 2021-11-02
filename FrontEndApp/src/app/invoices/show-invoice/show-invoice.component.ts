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
	public dataForm: FormGroup = new FormGroup({});
	public lineItemsDataForm: FormGroup;
	public focusedFieldId: string;

	constructor(private invoiceService: InvoiceService,
				private route: ActivatedRoute,
				private formBuilder: FormBuilder)
	{	
	}

	ngOnInit(): void
	{
		this.lineItemsDataForm = this.formBuilder.group({
			"lines": new FormArray([])
		});
		this.id = this.route.snapshot.paramMap.get('id');
		this.filters.push(new Filter("InvoiceId", "=", this.id));
		if (this.id)
		{
			this.invoiceService.getInvoice(this.id)
				.subscribe(data =>{
					this.invoiceData = data;
					for (var field in this.invoiceData)
					{
						this.dataForm.addControl(field, new FormControl(this.invoiceData[field], Validators.required));
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
		this.invoiceService.saveInvoice(this.dataForm.value, this.lineItemsDataForm.value)
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
		var id = (target as HTMLInputElement).id;
		this.focusedFieldId = id;
		console.log("Focused field: " + id);
	}

	fillExtractedText(text: string)
	{
		console.log("Text extracted into " + this.focusedFieldId + ": " + text);
		var input = document.getElementById(this.focusedFieldId) as HTMLInputElement;
		input.value = text;
		input.focus();
	}
}
