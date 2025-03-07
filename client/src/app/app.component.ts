import {Component, inject, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  title = 'client';
  http = inject(HttpClient);
  users : any;
  ngOnInit(): void {
    this.http.get("https://localhost:5001/api/users").subscribe({
      next: (resp) => this. users = resp,
      error: (err) => console.log(err),
      complete: () => console.log("Request was completed")
    });
  }


}
