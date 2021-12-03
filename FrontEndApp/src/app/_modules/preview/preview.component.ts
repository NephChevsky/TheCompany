import { Component, ElementRef, HostListener, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
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
	@Input()
	file: any;
	preview: SafeResourceUrl;
	extraction : ExtractBlock[];
	extractionSize : ExtractBlock;
	imgPosition: DOMRect = new DOMRect();
	dragPosition: Rectangle = new Rectangle(-1, -1, -1, -1);
	canvasCtx: CanvasRenderingContext2D;
	image: HTMLImageElement;
	selectedPosition: Rectangle = new Rectangle(-1, -1, -1, -1);
	selectedText: string = "";
	loading: boolean = true;

	@Output() extractedTextEvent = new EventEmitter<string>();

	constructor(private invoiceService: InvoiceService, private elementRef: ElementRef, private sanitizer:DomSanitizer)
	{
	}

	ngOnInit(): void
	{
		this.canvasCtx = (<HTMLCanvasElement> document.getElementById('canvasPreview')).getContext('2d');
		if (this.dataSource == "Invoice")
		{
			if (this.file != null)
			{
				this.invoiceService.getPreviewOnTheFly(this.file)
				.subscribe((data: any) =>{
					
					this.handlePreview(this.stringToArrayBuffer(data.preview));
					this.handleExtraction(this.stringToArrayBuffer(data.extraction));
					this.loading = false;
				}, error =>
				{
					// TODO: handle errors
				});
			}
			else if (this.id != null)
			{
				this.invoiceService.getPreview(this.id, this.page)
				.subscribe((data: any) =>{
					this.handlePreview(data);
					this.loading = this.extraction == null || this.preview == null;
				}, error =>
				{
					// TODO: handle errors
				});

				this.invoiceService.getExtraction(this.id)
				.subscribe((data: any) =>{
					this.handleExtraction(data);
					this.loading = this.extraction == null || this.preview == null;
				}, error =>
				{
					// TODO: handle errors
				});
			}
		}
	}

	stringToArrayBuffer(str: string)
	{
		var buf = new ArrayBuffer(str.length); // 2 bytes for each char
		var bufView = new Uint8Array(buf);
		for (var i=0, strLen=str.length; i < strLen; i++) {
			bufView[i] = str.charCodeAt(i);
		}
		return buf;
	}

	handlePreview(data: any)
	{
		var binary = '';
		var bytes = new Uint8Array( data );
		var len = bytes.byteLength;
		for (var i = 0; i < len; i++) {
			binary += String.fromCharCode(bytes[i]);
		}
		this.preview = this.sanitizer.bypassSecurityTrustResourceUrl("data:image/png;base64," + binary);
		let that = this;
		document.getElementById("imgPreview").addEventListener('load', function () {
			that.imgPosition = document.getElementById("imgPreview").getBoundingClientRect();
		});
	}

	handleExtraction(data: any)
	{
		var binary = '';
		var bytes = new Uint8Array( data );
		var len = bytes.byteLength;
		for (var i = 0; i < len; i++) {
			binary += String.fromCharCode(bytes[i]);
		}
		this.extraction = JSON.parse(decodeURIComponent(atob(binary).split('').map(function(c) {
			return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
		}).join('')));
		this.extractionSize = this.extraction[0];
		this.extraction.shift();
	}

	handleClick(event: PointerEvent)
	{
		if (event.offsetX >= this.selectedPosition.X && event.offsetX <= this.selectedPosition.X + this.selectedPosition.Width && event.offsetY >= this.selectedPosition.Y && event.offsetY <= this.selectedPosition.Y + this.selectedPosition.Height)
		{
			this.extractedTextEvent.emit(this.selectedText);
			this.selectedPosition = new Rectangle(-1,-1,-1,-1);
			this.selectedText = "";
		}
		else
		{
			this.startDrawing(event);
		}
	}

	startDrawing(event: PointerEvent)
	{
		if (this.extraction && this.extraction.length != 0)
		{
			this.selectedPosition = new Rectangle(-1,-1,-1,-1);
			this.selectedText = "";
			this.dragPosition = new Rectangle(event.offsetX,  event.offsetY, 0, 0);
			let canvasPosition = document.getElementById("canvasPreview").getBoundingClientRect();
			this.canvasCtx.clearRect(0, 0, canvasPosition.width, canvasPosition.height);
		}
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
		if (this.extraction && this.extraction.length != 0)
		{
			var result = this.drawBox(this.dragPosition);
			this.dragPosition = new Rectangle(-1, -1, -1, -1);

			this.selectedPosition = result.position;
			this.selectedText = result.text;
		}
	}

	drawBox(rect: Rectangle)
	{
		debugger;
		var result = this.extractText(rect);
		result.position = this.reversePosition(result.position);
		result.position.X = result.position.X - 3;
		result.position.Y = result.position.Y - 3;
		result.position.Width = result.position.Width + 6;
		result.position.Height = result.position.Height + 6;
		let canvasPosition = document.getElementById("canvasPreview").getBoundingClientRect();
		this.canvasCtx.clearRect(0, 0, canvasPosition.width, canvasPosition.height);
		this.canvasCtx.fillStyle = '#f9ff87';
		this.canvasCtx.fillRect(result.position.X, result.position.Y, result.position.Width, result.position.Height);
		this.canvasCtx.strokeStyle = 'red';
		this.canvasCtx.lineWidth = 1;
		this.canvasCtx.strokeRect(result.position.X, result.position.Y, result.position.Width, result.position.Height);
		
		for (var i = 0; i < result.words.length; i++)
		{
			var wordPosition = this.reversePosition(result.words[i]);
			this.canvasCtx.fillStyle = 'black';
			this.canvasCtx.fillText(result.words[i].Text, wordPosition.X, wordPosition.Y + wordPosition.Height);
		}
		return result;
	}

	@HostListener('window:resize', ['$event'])
	onResize() {
		this.imgPosition = document.getElementById("imgPreview").getBoundingClientRect();
	}

	extractText(position: Rectangle)
	{
		position = this.convertPosition(position);
		var resultTxt = "";
		var resultPosition = new Rectangle(-1,-1,-1,-1);
		var currentLine = -1;
		var words = [];
		for (var i = 1; i < this.extraction.length; i++)
		{
			var left = this.extraction[i].X + this.extraction[i].Width < position.X;
			var right = this.extraction[i].X > position.X + position.Width;
			var above = this.extraction[i].Y > position.Y + position.Height;
			var below = this.extraction[i].Y + this.extraction[i].Height < position.Y;
			if (!( left || right || above || below ))
			{
				words.push(this.extraction[i]);
				if (resultTxt)
				{
					if (this.extraction[i].Y > currentLine + 1)
						resultTxt += "\r\n";
					else
						resultTxt += " ";
				}
				if (currentLine == -1 || this.extraction[i].Y != currentLine)
					currentLine = this.extraction[i].Y;
				resultTxt += this.extraction[i].Text;
				if (resultPosition.X > this.extraction[i].X || resultPosition.X == -1)
					resultPosition.X = this.extraction[i].X;
				if (resultPosition.Y > this.extraction[i].Y || resultPosition.Y == -1)
					resultPosition.Y = this.extraction[i].Y;
				if (resultPosition.X + resultPosition.Width < this.extraction[i].X + this.extraction[i].Width || resultPosition.Width == -1)
					resultPosition.Width = this.extraction[i].X + this.extraction[i].Width - resultPosition.X;
				if (resultPosition.Y + resultPosition.Height < this.extraction[i].Y + this.extraction[i].Height || resultPosition.Height == -1)
					resultPosition.Height = this.extraction[i].Y + this.extraction[i].Height - resultPosition.Y;
			}
		}

		if (resultPosition.X == -1)
		{
			resultPosition = position;
		}

		var result = {
			text: resultTxt,
			position: resultPosition,
			words: words
		}
		return result;
	}

	convertPosition(position: Rectangle)
	{
		var pos = new Rectangle(position.X * this.extractionSize.Width / this.imgPosition.width,
								position.Y * this.extractionSize.Height / this.imgPosition.height,
								position.Width * this.extractionSize.Width / this.imgPosition.width,
								position.Height * this.extractionSize.Height / this.imgPosition.height);
		return pos;
	}

	reversePosition(position: Rectangle)
	{
		var pos = new Rectangle(position.X * this.imgPosition.width / this.extractionSize.Width,
			position.Y * this.imgPosition.height / this.extractionSize.Height,
			position.Width * this.imgPosition.width / this.extractionSize.Width,
			position.Height * this.imgPosition.height / this.extractionSize.Height);
		return pos;
	}
}
