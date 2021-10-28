import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

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

	saveExtractionSettings(data: any)
	{
		return this.http.post(environment.baseUrl + `Invoice/SaveExtractionSettings`, data);
	}

	getExtractionSettings(ids: string[] = [])
	{
		return this.http.post<any>(environment.baseUrl + `Invoice/GetExtractionSettings`, ids);
	}

	getInvoice(id: string)
	{
		return this.http.get<any>(environment.baseUrl + `Invoice/Show/` + id);
	}

	getPreview(id: string, page: number)
	{
		return this.http.get(environment.baseUrl + `Invoice/Preview/` + id + "/" + page, {responseType: 'arraybuffer'});
	}
}
