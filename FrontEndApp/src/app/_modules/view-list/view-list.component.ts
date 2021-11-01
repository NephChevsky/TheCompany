import { Component, Input, OnInit } from '@angular/core';
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
	public addLine: boolean = false;

	public linkable: boolean = false;
	public fieldsName: string[] = [];
	public data: any[] = [];
	public addedDataForm: FormGroup = new FormGroup({});

	constructor(private viewListService: ViewListService,
				private router: Router,
				private formBuilder: FormBuilder) {
	}

	ngOnInit(): void
	{
		this.addedDataForm = this.formBuilder.group({
			"lines": new FormArray([])
		});

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
			reference: ['', []],
			description: ['', []],
			quantity: ['', []],
			unitaryprice: ['', []],
			price: ['',[]]
		});
		this.lines.push(tmpForm);
	}

	get lines()
	{
		return this.addedDataForm.controls["lines"] as FormArray;
	}
}
