import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  postMethodRegistration(user): Observable<any> {
    return this.httpClient.post(environment.endpointAccountRegister, user)
  }

  postMethodApproveAccount(uploadedImage: File): Observable<any> {
    this.formData = new FormData();
    
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post(environment.endpointAccountFinish, this.formData, { headers: headers });
  }

}
