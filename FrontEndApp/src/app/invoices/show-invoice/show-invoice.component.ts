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
	preview: any;

	constructor(private invoiceService: InvoiceService,
				private route: ActivatedRoute,
				private domSanitizer: DomSanitizer) { }

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

			this.invoiceService.getPreview(this.id, 1)
				.subscribe((data: any) =>{
					var binary = '';
					var bytes = new Uint8Array( data );
					var len = bytes.byteLength;
					for (var i = 0; i < len; i++) {
						binary += String.fromCharCode( bytes[ i ] );
					}
					this.preview = "data:image/png;base64," + binary;
				}, error =>
				{
					// TODO: handle errors
				});
		}
	}
}
