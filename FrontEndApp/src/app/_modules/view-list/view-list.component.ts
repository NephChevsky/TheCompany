import { Component, Input, OnInit } from '@angular/core';
import { ViewListService } from 'src/app/_services/view-list.service';

@Component({
  selector: 'app-view-list',
  templateUrl: './view-list.component.html',
  styleUrls: ['./view-list.component.scss']
})
export class ViewListComponent implements OnInit {

	@Input()
    public dataSource: string = "";

	public fields: string[] = [];
	public data: any[] = [];

	constructor(private viewListService: ViewListService) {
	}

	ngOnInit(): void
	{
		this.viewListService.getResults(this.dataSource).subscribe(x => {
			this.fields = x[0];
			x.shift();
			this.data = x;
		});
	}
}
