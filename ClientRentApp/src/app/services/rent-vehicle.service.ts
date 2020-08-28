import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment'
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import { env } from 'process';

@Injectable({
  providedIn: 'root'
})
export class RentVehicleService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getService(serviceId): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleGetService + serviceId);
  }

  getMethodServices(): Observable<any> {
    return this.httpClient.get(environment.endpointRentVehicleGetServices)
  }

  getMethodComments(serviceId : string){
    return this.httpClient.get(environment.endpointBookingGetCommentsForService + serviceId);
  }

  getMethodRatings(serviceId : string){
    return this.httpClient.get(environment.endpointBookingGetRatingsForService + serviceId);
  }

  getMethodServicesForApproves(): Observable<any> {
    return this.httpClient.get(environment.endpointRentVehicleGetServicesForApproval);
  }

  getMethodHasUserCommented(serviceId : string){
    return this.httpClient.get(environment.endpointBookingHasUserCommented + serviceId);
  }
  
  getMethodHasUserRated(serviceId : string){
    return this.httpClient.get(environment.endpointBookingHasUserRated + serviceId);
  }
  
  deleteService(serviceId): Observable<any>{
    return this.httpClient.delete(environment.endpointRentVehicleDeleteService + serviceId);
  }

  postMethodApproveService(serviceId : number) {
    return this.httpClient.post(environment.endpointRentVehicleApproveService, serviceId);
  }

  postMethodRejectService(serviceId : number) {
    return this.httpClient.post(environment.endpointRentVehicleRejectService, serviceId);
  }

  postMethodCreateRentVehicleService(rentVehicle, uploadedImage: File): Observable<any> {
    this.formData = new FormData();

    this.formData.append('name', rentVehicle.name)
    this.formData.append('contactEmail', rentVehicle.contactEmail);
    this.formData.append('description', rentVehicle.description);
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'multipart/form-data')

    return this.httpClient.post(environment.endpointRentVehicleCreateService, this.formData, { headers: headers });
  }

  editService(serviceId, service, uploadedImage: File): Observable<any>{
    this.formData = new FormData();
    
    this.formData.append('id', serviceId);
    this.formData.append('serviceId', serviceId);
    this.formData.append('name', service.Name)
    this.formData.append('emailaddress', service.EmailAddress);
    this.formData.append('description', service.Description);
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put(environment.endpointRentVehicleEditService, this.formData, { headers: headers });
  }

}