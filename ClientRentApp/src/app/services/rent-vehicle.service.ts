import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class RentVehicleService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  postMethodCreateRentVehicleService(rentVehicle, uploadedImage: File): Observable<any> {
    this.formData.append('name', rentVehicle.name)
    this.formData.append('contactEmail', rentVehicle.contactEmail);
    this.formData.append('description', rentVehicle.description);
    this.formData.append('logoImage', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'multipart/form-data')

    let result = this.httpClient.post("http://localhost:51680/api/Services",this.formData, { headers: headers });

    this.formData = new FormData();

    return result;
  }

  getMethodServices(): Observable<any> {
    return this.httpClient.get("http://localhost:51680/api/Services")
  }

}