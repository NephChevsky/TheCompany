import { Component, ElementRef, HostListener, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ExtractBlock } from 'src/app/_models/extractBlock';
import { Rectangle } from 'src/app/_models/rectangle';
import { InvoiceService } from 'src/app/_services/invoice.service';

@Component({
  selector: 'app-preview',
  templateUrl: './preview.component.html',
  styleUrls: ['./preview.component.scss']
})
export class PreviewComponent implements OnInit
{
	@Input()
	dataSource: string = "";
	@Input()
	id: string = "";
	@Input()
	page: number = 1;
	preview: string;
	extraction : ExtractBlock[];
	imgPosition: DOMRect = new DOMRect();
	dragPosition: Rectangle = new Rectangle(-1, -1, -1, -1);
	canvasCtx: CanvasRenderingContext2D;
	image: HTMLImageElement;

	@Output() extractedTextEvent = new EventEmitter<string>();

	constructor(private invoiceService: InvoiceService, private elementRef: ElementRef)
	{
	}

	ngOnInit(): void
	{
		this.canvasCtx = (<HTMLCanvasElement> document.getElementById('canvasPreview')).getContext('2d');
		if (this.dataSource == "Invoice")
		{
			this.invoiceService.getPreview(this.id, this.page)
				.subscribe((data: any) =>{
					var binary = '';
					var bytes = new Uint8Array( data );
					var len = bytes.byteLength;
					for (var i = 0; i < len; i++) {
						binary += String.fromCharCode( bytes[ i ] );
					}
					this.preview = "data:image/png;base64," + binary;
					let that = this;
					document.getElementById("imgPreview").addEventListener('load', function () {
						that.imgPosition = document.getElementById("imgPreview").getBoundingClientRect();
					});
					
				}, error =>
				{
					// TODO: handle errors
				});
			this.invoiceService.getExtraction(this.id)
				.subscribe((data: any) =>{
					var binary = '';
					var bytes = new Uint8Array( data );
					var len = bytes.byteLength;
					for (var i = 0; i < len; i++) {
						binary += String.fromCharCode( bytes[ i ] );
					}
					this.extraction = JSON.parse(atob(binary));
				}, error =>
				{
					// TODO: handle errors
				});
		}
	}

	startDrawing(event: PointerEvent)
	{
		this.dragPosition = new Rectangle(event.offsetX,  event.offsetY, 0, 0);
		let canvasPosition = document.getElementById("canvasPreview").getBoundingClientRect();
		this.canvasCtx.clearRect(0, 0, canvasPosition.width, canvasPosition.height);
	}

	draw(event: PointerEvent)
	{
		if (this.dragPosition.X != -1)
		{
			this.dragPosition.Width = event.offsetX - this.dragPosition.X;
			this.dragPosition.Height = event.offsetY - this.dragPosition.Y;
			let canvasPosition = document.getElementById("canvasPreview").getBoundingClientRect();
			this.canvasCtx.clearRect(0, 0, canvasPosition.width, canvasPosition.height);
			this.canvasCtx.strokeStyle = 'red';
			this.canvasCtx.strokeRect(this.dragPosition.X, this.dragPosition.Y, this.dragPosition.Width, this.dragPosition.Height);
		}
	}

	stopDrawing(event: PointerEvent)
	{
		this.extractText(this.dragPosition);
		this.dragPosition = new Rectangle(-1, -1, -1, -1);
	}

	@HostListener('window:resize', ['$event'])
	onResize() {
		this.imgPosition = document.getElementById("imgPreview").getBoundingClientRect();
	}

	extractText(position: Rectangle)
	{
		position = this.translatePosition(position);
		var result ="";
		for (var i = 1; i < this.extraction.length; i++)
		{
			var left = this.extraction[i].X + this.extraction[i].Width < position.X;
			var right = this.extraction[i].X > position.X + position.Width;
			var above = this.extraction[i].Y > position.Y + position.Height;
			var below = this.extraction[i].Y + this.extraction[i].Height < position.Y;
			if (!( left || right || above || below ))
			{
				if (result)
					result += " ";
				result += this.extraction[i].Text;
			}
		}
		this.extractedTextEvent.emit(result);
	}

	translatePosition(position: Rectangle)
	{
		var extractionSize = this.extraction[0];
		var pos = new Rectangle(position.X * extractionSize.Width / this.imgPosition.width,
								position.Y * extractionSize.Height / this.imgPosition.height,
								position.Width * extractionSize.Width / this.imgPosition.width,
								position.Height * extractionSize.Height / this.imgPosition.height);
		return pos;
	}
}
