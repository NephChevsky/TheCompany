import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CompanyService
{
	constructor(private http: HttpClient)
	{
	}

	get()
	{
		return this.http.get<any>(environment.baseUrl + `Company/Get`);
	}

	save(obj: any)
	{
		return this.http.post<any>(environment.baseUrl + `Company/Save`, obj);
	}

	saveLogo(obj: any)
	{
		return this.http.post<any>(environment.baseUrl + `Company/SaveLogo`, obj);
	}
}
