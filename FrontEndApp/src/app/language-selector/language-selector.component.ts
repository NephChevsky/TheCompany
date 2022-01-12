import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
	selector: 'app-language-selector',
	templateUrl: './language-selector.component.html',
	styleUrls: ['./language-selector.component.scss'],
	encapsulation: ViewEncapsulation.None
})
export class LanguageSelectorComponent implements OnInit
{
	public countries: any[] = [
		{ value: "en", viewValue: "Languages.EN", image: "/assets/country-images/en.svg" },
		{ value: "fr", viewValue: "Languages.FR", image: "/assets/country-images/fr.svg" }
	];
	@Input()
	public value: string;

	constructor() { }

	ngOnInit(): void
	{
		
	}

	onChange(language: string)
	{
		localStorage.setItem('selectedLanguage', language);
		window.location.reload();
	}
}
