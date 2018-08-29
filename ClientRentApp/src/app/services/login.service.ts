import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private httpClient: HttpClient) { }

  postMethodLogin(user) : Observable<any>
  {
      let headers = new HttpHeaders();
      
      headers = headers.append('Content-type', 'application/x-www-form-urlencoded');
      
      return this.httpClient.post('https://localhost:44365/oauth/token',`username=${user.email}&password=${user.password}&grant_type=password`, {"headers": headers}) as Observable<any>
  }
}