import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  postMethodRegistration(user): Observable<any> {
    return this.httpClient.post("https://localhost:5001/api/Account/Register", user)
  }

  postMethodApproveAccount(uploadedImage: File): Observable<any> {
    this.formData = new FormData();
    
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post("https://localhost:5001/api/Account/FinishAccount", this.formData, { headers: headers });
  }

}
