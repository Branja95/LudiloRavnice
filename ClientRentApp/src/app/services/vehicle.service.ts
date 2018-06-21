import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getMethodVehicleTypes(): Observable<any> {
    return this.httpClient.get("http://localhost:51680/api/VehicleTypes")
  }

  postMethodCreateVehicle(vehicle, uploadedImages: FileList): Observable<any> {
    this.formData.append('serviceId', vehicle.serviceId);
    this.formData.append('vehicleTypeId', vehicle.vehicleType);
    this.formData.append('model', vehicle.model);
    this.formData.append('manufactor', vehicle.manufactor);
    this.formData.append('vehicleType', vehicle.vehicleType)
    this.formData.append('yearMade', vehicle.yearMade)
    this.formData.append('description', vehicle.description);
    this.formData.append('pricePerHour', vehicle.pricePerHour);
    this.formData.append('isAvailable', vehicle.isAvailable);
    
    Array.from(uploadedImages).forEach(uploadedImage => { 
      this.formData.append(uploadedImage.name, uploadedImage, uploadedImage.name);
    });

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result = this.httpClient.post("http://localhost:51680/api/Vehicles", this.formData, { headers: headers });

    this.formData = new FormData();

    return result;

  }

  getMethodVehicles(): Observable<any> {
    return this.httpClient.get("http://localhost:51680/api/Vehicles")
  }

}