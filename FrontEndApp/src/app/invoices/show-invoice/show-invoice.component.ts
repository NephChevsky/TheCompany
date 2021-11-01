import { Component, OnInit, SecurityContext } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
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

	constructor(private invoiceService: InvoiceService,
				private route: ActivatedRoute) { }

	ngOnInit(): void {
		this.id = this.route.snapshot.paramMap.get('id');
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
