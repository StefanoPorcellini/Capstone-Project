import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  private apiUrl = "https://localhost:7147/api";

  constructor(private http: HttpClient) { }

}
