import { Component, OnInit, SecurityContext } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Filter } from 'src/app/_models/filter';
import { InvoiceService } from 'src/app/_services/invoice.service';

@Component({
  selector: 'app-show-invoice',
  templateUrl: './show-invoice.component.html',
  styleUrls: ['./show-invoice.component.scss']
})
export class ShowInvoiceComponent implements OnInit {

	id: string = "";
	invoiceData: any;
	page: number = 1;
	public filters: Filter[] = [];

	constructor(private invoiceService: InvoiceService,
				private route: ActivatedRoute) { }

	ngOnInit(): void {
		this.id = this.route.snapshot.paramMap.get('id');
		this.filters.push(new Filter("InvoiceId", "=", this.id));
		if (this.id)
		{
			this.invoiceService.getInvoice(this.id)
				.subscribe(data =>{
					this.invoiceData = data;
				}, error =>
				{
					// TODO: handle errors
				});
		}
	}
}
