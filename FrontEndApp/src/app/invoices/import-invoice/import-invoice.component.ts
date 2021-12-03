import { HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { InvoiceService } from 'src/app/_services/invoice.service';

@Component({
  selector: 'app-import-invoice',
  templateUrl: './import-invoice.component.html',
  styleUrls: ['./import-invoice.component.scss']
})
export class ImportInvoiceComponent implements OnInit {

	public error: string = "";
	form: FormGroup = new FormGroup({});
	progress: number = 0;
	inProgress: boolean = false;

	constructor (private formBuilder: FormBuilder,
				private invoiceService: InvoiceService,
				private router: Router)
	{
		
	}

	ngOnInit()
	{
		this.form = this.formBuilder.group({
			file: [null]
		});
	}

	onFileChange(event: any)
	{
		if (event.target.files.length > 0)
		{
			const file = event.target.files[0];
			var fileObj = this.form.get('file');
			if (fileObj)
				fileObj.setValue(file);
		}
		else
		{
			var fileObj = this.form.get('file');
			if (fileObj)
				fileObj.setValue(null);
		}
	}

	submit()
	{
		const formData = new FormData();
		var fileObj = this.form.get('file')
		if (fileObj)
		{
			if (!fileObj.value)
			{
				this.error = "Required";
				return;
			}
			formData.append('file', fileObj.value);
			this.inProgress = true;
			this.invoiceService.import(formData).subscribe((event: any) =>
			{
				switch (event.type)
				{
					case HttpEventType.UploadProgress:
						this.progress = Math.round(100 * event.loaded / event.total);
						return;
					case HttpEventType.Response:
						this.inProgress = false;
						this.router.navigate(['Invoices']);
						return;
					case HttpEventType.Sent:
						return;
				}
			}, (err: any) =>
			{
				this.inProgress = false;
				if (err.status === 500)
					this.error = "UnknownError"
				else
					this.error = err.error;
			});
		}
	}

	onCancel()
	{
		this.router.navigate(['Invoices']);
	}

	get errorMessage(): string
	{
		let message = "";
		if (this.error === "Required")
		{
			message = "You have to select a file\r\n";
		}
		else if (this.error === "AlreadyExists")
		{
			message = "The file name already exists\r\n";
		}
		else if (this.error === "InvalidCharacters")
		{
			message = "Some characters are not allowed\r\n";
		}
		else if (this.error === "UnknownError")
		{
			message = "Unknown error\r\n";
		}
		else if (this.error === 'UnknownParentFolder')
		{
			message = "The parent folder doesn't exists\r\n";
		}
		return message;
	}
}
