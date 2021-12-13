import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, Form, FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Filter } from 'src/app/_models/filter';
import { ViewListService } from 'src/app/_services/view-list.service';
import { Field } from '../../_models/field';

@Component({
	selector: 'app-view-list',
	templateUrl: './view-list.component.html',
	styleUrls: ['./view-list.component.scss']
})
export class ViewListComponent implements OnInit
{

	@Input()
	public dataSource: string = "";
	@Input()
	public linkable: boolean = false;
	@Input()
	public linkRoute: string = "";
	@Input()
	public filters: Filter[] = [];
	@Input()
	public fields: string[] = [];
	@Input()
	public editMode: boolean = false;
	@Input()
	public linesForm: FormGroup;
	@Output() focusEvent = new EventEmitter<EventTarget>();

	public data: any[] = [];
	public fieldsData: Field[] = [];

	constructor(private viewListService: ViewListService,
		private router: Router,
		private formBuilder: FormBuilder)
	{
	}

	ngOnInit(): void
	{
		if (this.linesForm == null)
		{
			this.linesForm = new FormGroup({});
		}

		if (this.linesForm.get("values") == null)
		{
			this.linesForm.addControl("values", this.formBuilder.array([]));
		}

		this.viewListService.getResults(this.dataSource, this.filters, this.fields).subscribe(x =>
		{
			this.data = x.items;
			this.fieldsData = x.fieldsData;
			var items = this.linesForm.get("values") as FormArray;
			for (var i = 0; i < this.data.length; i++)
			{
				var controls = this.formBuilder.group({});
				controls.addControl("Id", new FormControl(this.data[i].linkValue));
				for (var j = 0; j < this.data[i].fields.length; j++)
				{
					var field = (this.data[i].fields[j] as Field);
					controls.addControl(this.data[i].fields[j].name, new FormControl(this.data[i].fields[j].value));
				}
				items.push(controls);
			}
		});
	}

	show(id: string)
	{
		this.router.navigate([this.linkRoute + id]);
	}

	createNewLine()
	{
		const tmpForm = this.formBuilder.group({});
		tmpForm.addControl("Id", new FormControl(""));
		for (var i = 0; i < this.fields.length; i++)
		{
			tmpForm.addControl(this.fields[i], new FormControl(""));
		}
		(this.linesForm.get("values") as FormArray).push(tmpForm);
	}

	get values()
	{
		return this.linesForm.controls["values"] as FormArray;
	}

	getItem(index: number)
	{
		return (this.linesForm.controls["values"] as FormArray).at(index) as FormGroup;
	}

	getField(name: string) : Field
	{
		return this.fieldsData.find(x => x.name == name);
	}

	onFocus(target: EventTarget)
	{
		if (this.editMode)
			this.focusEvent.emit(target);
	}
}
