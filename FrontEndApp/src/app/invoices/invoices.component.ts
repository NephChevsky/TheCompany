import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-invoices',
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.scss']
})
export class InvoicesComponent implements OnInit {

	public dataSource: string = "Invoice";
	public linkField: string = "Id";
	public linkRoute: string = "Invoices/Show/"
	constructor(private router: Router) { }

	ngOnInit(): void {
	}
}
