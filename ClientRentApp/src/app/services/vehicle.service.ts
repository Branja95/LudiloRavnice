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

  getVehicleType(vehhicleTypeId):Observable<any>{
    return this.httpClient.get("http://localhost:51680/api/VehicleTypes/" + vehhicleTypeId);
  }

  getVehicleTypes(): Observable<any> {
    return this.httpClient.get("http://localhost:51680/api/VehicleTypes")
  }

  createVehicle(vehicle, uploadedImages: FileList): Observable<any> {
    this.formData.append('serviceId', vehicle.serviceId);
    this.formData.append('vehicleTypeId', vehicle.vehicleType);
    this.formData.append('model', vehicle.model);
    this.formData.append('manufactor', vehicle.manufactor);
    this.formData.append('yearMade', vehicle.yearMade)
    this.formData.append('description', vehicle.description);
    this.formData.append('pricePerHour', vehicle.pricePerHour);
    this.formData.append('isAvailable', vehicle.isAvailable);
    
    Array.from(uploadedImages).forEach(uploadedImage => { 
      this.formData.append(uploadedImage.name, uploadedImage, uploadedImage.name);
    });

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result = this.httpClient.post("http://localhost:51680/api/Vehicles/PostVehicle", this.formData, { headers: headers });

    this.formData = new FormData();

    return result;

  }

  getVehicles(): Observable<any> {
    return this.httpClient.get("http://localhost:51680/api/Vehicles/GetVehicles")
  }

  getServiceVehicles(serviceId): Observable<any> {
    return this.httpClient.get("http://localhost:51680/api/Services/GetVehicles?serviceId=" + serviceId);
  }

  getVehicle(vehicleId): Observable<any>{
    return this.httpClient.get("http://localhost:51680/api/Vehicles/GetVehicle?id=" + vehicleId);
  }

  deleteVehicle(vehicleId): Observable<any> {
    return this.httpClient.delete("http://localhost:51680/api/Vehicles/DeleteVehicle?id=" + vehicleId);
  }

  editVehicle(vehicleId, vehicle, uploadedImages: FileList): Observable<any>{
    this.formData.append('id', vehicleId);
    this.formData.append('vehicleTypeId', vehicle.vehicleType);
    this.formData.append('model', vehicle.model);
    this.formData.append('manufactor', vehicle.manufactor);
    this.formData.append('yearMade', vehicle.yearMade)
    this.formData.append('description', vehicle.description);
    this.formData.append('pricePerHour', vehicle.pricePerHour);
    this.formData.append('isAvailable', vehicle.isAvailable);
    Array.from(uploadedImages).forEach(uploadedImage => { 
      this.formData.append(uploadedImage.name, uploadedImage, uploadedImage.name);
    });

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result = this.httpClient.put("http://localhost:51680/api/Vehicles/PutVehicle?id=" + vehicleId, this.formData, { headers: headers });

    this.formData = new FormData();

    return result;
  }
}