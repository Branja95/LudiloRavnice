import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { environment } from '../../environments/environment';
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
    return this.httpClient.get(environment.endpointRentVehicleVehicleTypes + vehhicleTypeId);
  }

  getVehicleTypes(): Observable<any> {
    return this.httpClient.get(environment.endpointRentVehicleGetVehicleTypes);
  }

  getVehicles(): Observable<any> {
    return this.httpClient.get(environment.endpointRentVehicleGetVehicles);
  }

  getServiceVehicles(serviceId): Observable<any> {
    return this.httpClient.get(environment.endpointRentVehicleGetServiceVehicles + serviceId);
  }

  getVehicle(vehicleId): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleGetVehicle + vehicleId);
  } 
  
  getNumberOfVehicles(): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleGetNumberOfVehicles);
  }

  getPagedVehicles(pageIndex, pageSize): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleGetPagedVehicles + pageIndex + "&pageSize=" + pageSize);
  }

  getSearchPagedVehicles(pageIndex, pageSize, vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleGetSearchPagedVehicles + pageIndex + "&pageSize=" + pageSize + "&vehicleTypeId=" + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);
  }

  searchNumberOfVehicles(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleSearchNumberOfVehicles + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);   
  }

  searchVehicles(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleSearchVehicles + vehicleTypeId + "&vehiclePriceFrom=" + vehiclePriceFrom + "&vehiclePriceTo=" + vehiclePriceTo + "&vehicleManufactor=" + vehicleManufactor + "&vehicleModel=" + vehicleModel);   
  }

  deleteVehicle(vehicleId, serviceId): Observable<any> {
    return this.httpClient.delete(environment.endpointRentVehicleDeleteVehicle + vehicleId + "&serviceId=" + serviceId);
  }

  changeAvailability(vehicleId){

    this.formData = new FormData();
    this.formData.append('vehicleId', vehicleId);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');

    return this.httpClient.put(environment.endpointRentVehicleChangeAvailability, this.formData, { headers: headers });
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
      this.formData.append('images', uploadedImage, uploadedImage.name);
    });

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result = this.httpClient.post(environment.endpointRentVehicleCreateVehicle, this.formData, { headers: headers });

    this.formData = new FormData();

    return result;
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

    if(uploadedImages != null){
      Array.from(uploadedImages).forEach(uploadedImage => { 
        this.formData.append('images', uploadedImage, uploadedImage.name);
      });
    }

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result = this.httpClient.put(environment.endpointRentVehicleEditVehicle, this.formData, { headers: headers });
  
    this.formData = new FormData();
    return result;
  }
}