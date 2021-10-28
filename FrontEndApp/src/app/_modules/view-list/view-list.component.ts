import { ThisReceiver } from '@angular/compiler';
import { Component, Input, OnInit } from '@angular/core';
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

	public linkable: boolean = false;
	public fields: string[] = [];
	public data: any[] = [];

	constructor(private viewListService: ViewListService,
				private router: Router) {
	}

	ngOnInit(): void
	{
		if (this.linkField)
		{
			this.linkable = true;
		}

		this.viewListService.getResults(this.dataSource, this.filters).subscribe(x => {
			this.fields = x[0];
			x.shift();
			this.data = x;
		});
	}

	show(id: string)
	{
		this.router.navigate([this.linkRoute + id]);
	}
}
