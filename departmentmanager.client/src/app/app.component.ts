import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [HttpClient]
})
export class AppComponent implements OnInit {
  httpClient = inject(HttpClient);
  constructor() {}

  ngOnInit() {
    this.httpClient.get('http://localhost:5047/weatherforecast').subscribe(x => console.log(x));
  }


  title = 'departmentmanager.client';
}
