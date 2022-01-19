import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class InvoiceService
{
	constructor(private http: HttpClient)
	{
		
	}

	import(data: any)
	{
		return this.http.post(environment.baseUrl + `Invoice/Import`, data, {
		reportProgress: true,
		observe: 'events'
		})
	}

	saveExtractionSettings(invoiceSettings: any, lineItemSettings: any)
	{
		var data = {
			invoiceSettings: invoiceSettings.fields,
			lineItemSettings: lineItemSettings.fields
		}
		return this.http.post(environment.baseUrl + `Invoice/SaveExtractionSettings`, data);
	}

	getExtractionSettings(ids: string[] = [])
	{
		return this.http.post(environment.baseUrl + `Invoice/GetExtractionSettings`, ids);
	}

	getInvoice(id: string)
	{
		return this.http.get(environment.baseUrl + `Invoice/Show/` + id);
	}

	getPreview(id: string, page: number)
	{
		return this.http.get(environment.baseUrl + `Invoice/GetPreview/` + id + "/" + page, {responseType: 'arraybuffer'});
	}

	getPreviewOnTheFly(data: any)
	{
		return this.http.post(environment.baseUrl + `Invoice/GetPreviewOnTheFly`, data);
	}

	getExtraction(id: string)
	{
		return this.http.get(environment.baseUrl + `Invoice/Extraction/` + id, {responseType: 'arraybuffer'});
	}

	save(obj: any)
	{
		return this.http.post<any>(environment.baseUrl + `Invoice/Save`, obj);
	}
}
