import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
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
	public editMode: boolean = false;
	@Input()
	public addedLines: FormGroup;
	@Output() focusEvent = new EventEmitter<EventTarget>();

	public linkable: boolean = false;
	public fieldsName: string[] = [];
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

		this.viewListService.getResults(this.dataSource, this.filters).subscribe(x => {
			this.fieldsName = x[0];
			x.shift();
			this.data = x;
		});
	}

	show(id: string)
	{
		this.router.navigate([this.linkRoute + id]);
	}

	createNewLine()
	{
		const tmpForm = this.formBuilder.group({
			reference: ['1', []],
			description: ['2', []],
			quantity: ['3', []],
			unitaryprice: ['4', []],
			price: ['5',[]]
		});
		this.lines.push(tmpForm);
	}

	get lines()
	{
		if (!this.addedLines)
			this.addedLines = this.formBuilder.group({
				"lines": new FormArray([])
			});
		return this.addedLines.controls["lines"] as FormArray;
	}

	onFocus(target: EventTarget)
	{
		this.focusEvent.emit(target);
	}
}
