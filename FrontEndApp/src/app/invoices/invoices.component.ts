import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-invoices',
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.scss']
})
export class InvoicesComponent implements OnInit {

	public dataSource: string = "Invoices";
	public linkField: string = "Id;"
	constructor(private router: Router) { }

	ngOnInit(): void {
	}

	importInvoice() {
		this.router.navigate(['invoices/import']);
	}
}
