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
    return this.httpClient.get("https://localhost:44365/api/VehicleTypes/" + vehhicleTypeId);
  }

  getVehicleTypes(): Observable<any> {
    return this.httpClient.get("https://localhost:44365/api/VehicleTypes/GetVehicleTypes")
  }

  getVehicles(): Observable<any> {
    return this.httpClient.get("https://localhost:44365/api/Vehicles/GetVehicles")
  }

  getServiceVehicles(serviceId): Observable<any> {
    return this.httpClient.get("https://localhost:44365/api/Services/GetVehicles?serviceId=" + serviceId);
  }

  getVehicle(vehicleId): Observable<any>{
    return this.httpClient.get("https://localhost:44365/api/Vehicles/GetVehicle?id=" + vehicleId);
  } 
  
  getNumberOfVehicles(): Observable<any>{
    return this.httpClient.get("https://localhost:44365/api/Vehicles/GetNumberOfVehicles");
  }

  getPagedVehicles(pageIndex, pageSize): Observable<any>{
    return this.httpClient.get("https://localhost:44365/api/Vehicles/GetPagedVehicles?pageIndex=" + pageIndex + "&pageSize=" + pageSize);
  }

  getSearchPagedVehicles(pageIndex, pageSize, vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get("https://localhost:44365/api/Vehicles/GetSearchPagedVehicles?pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&vehicleTypeId=" + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);
  }

  searchNumberOfVehicles(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get("https://localhost:44365/api/Vehicles/SearchNumberOfVehicles?vehicleTypeId=" + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);   
  }

  searchVehicles(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get("https://localhost:44365/api/Vehicles/SearchVehicles?vehicleTypeId=" + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);   
  }

  deleteVehicle(vehicleId): Observable<any> {
    return this.httpClient.delete("https://localhost:44365/api/Vehicles/DeleteVehicle?id=" + vehicleId);
  }

  createVehicle(vehicle, uploadedImages: FileList): Observable<any> {
    this.formData = new FormData();

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

    return this.httpClient.post("https://localhost:44365/api/Vehicles/PostVehicle", this.formData, { headers: headers });
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

    return this.httpClient.put("https://localhost:44365/api/Vehicles/PutVehicle?id=" + vehicleId, this.formData, { headers: headers });
  }

}