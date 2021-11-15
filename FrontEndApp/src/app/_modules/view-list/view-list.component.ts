import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, Form, FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Filter } from 'src/app/_models/filter';
import { ViewListService } from 'src/app/_services/view-list.service';

@Component({
  selector: 'app-view-list',
  templateUrl: './view-list.component.html',
  styleUrls: ['./view-list.component.scss']
})
export class ViewListComponent implements OnInit {

	@Input()
    public dataSource: string = "";
	@Input()
	public linkField: string = "";
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

	public linkable: boolean = false;
	public data: any[] = [];

	constructor(private viewListService: ViewListService,
				private router: Router,
				private formBuilder: FormBuilder) {
	}

	ngOnInit(): void
	{
		if (this.linkField)
		{
			this.linkable = true;
		}

		this.viewListService.getResults(this.dataSource, this.filters, this.fields).subscribe(x => {
			this.data = x;

			if (this.linesForm)
			{
				this.linesForm.addControl("values", this.formBuilder.array([]));
				var items = this.linesForm.get("values") as FormArray;
				for (var i = 0; i < x.length; i++)
				{
					var controls = this.formBuilder.group({});
					for (const [key, value] of Object.entries(x[i]))
					{
						controls.addControl(key, new FormControl(value));
					}
					items.push(controls);
				}
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

	onFocus(target: EventTarget)
	{
		if (this.editMode)
			this.focusEvent.emit(target);
	}
}
