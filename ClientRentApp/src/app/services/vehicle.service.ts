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
    return this.httpClient.get("https://localhost:44367/api/VehicleTypes/" + vehhicleTypeId);
  }

  getVehicleTypes(): Observable<any> {
    return this.httpClient.get("https://localhost:44367/api/VehicleType/GetVehicleTypes")
  }

  getVehicles(): Observable<any> {
    return this.httpClient.get("https://localhost:44367/api/Vehicle/GetVehicles")
  }

  getServiceVehicles(serviceId): Observable<any> {
    return this.httpClient.get("https://localhost:44367/api/Service/GetVehicles?serviceId=" + serviceId);
  }

  getVehicle(vehicleId): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/Vehicle/GetVehicle?id=" + vehicleId);
  } 
  
  getNumberOfVehicles(): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/Vehicle/GetNumberOfVehicles");
  }

  getPagedVehicles(pageIndex, pageSize): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/Vehicle/GetPagedVehicles?pageIndex=" + pageIndex + "&pageSize=" + pageSize);
  }

  getSearchPagedVehicles(pageIndex, pageSize, vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/Vehicle/GetSearchPagedVehicles?pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&vehicleTypeId=" + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);
  }

  searchNumberOfVehicles(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/Vehicle/SearchNumberOfVehicles?vehicleTypeId=" + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);   
  }

  searchVehicles(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/Vehicle/SearchVehicles?vehicleTypeId=" + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);   
  }

  deleteVehicle(vehicleId): Observable<any> {
    return this.httpClient.delete("https://localhost:44367/api/Vehicle/DeleteVehicle?id=" + vehicleId);
  }

  changeAvailability(vehicleId){

    this.formData = new FormData();
    this.formData.append('vehicleId', vehicleId);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');

    return this.httpClient.put("https://localhost:44367/api/Vehicle/ChangeAvailability", this.formData, { headers: headers });
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

    let result = this.httpClient.post("https://localhost:44367/api/Vehicle/PostVehicle", this.formData, { headers: headers });

    this.formData = new FormData();

    return result;
  }

  editVehicle(vehicleId, vehicle, uploadedImages: FileList): Observable<any>{
    this.formData = new FormData();

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

    return this.httpClient.put("https://localhost:44367/api/Vehicle/PutVehicle?id=" + vehicleId, this.formData, { headers: headers });
  }

}