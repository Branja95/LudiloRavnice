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

  getService(serviceId): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/Service/GetService?serviceId=" + serviceId);
  }

  getMethodServices(): Observable<any> {
    return this.httpClient.get("https://localhost:44367/api/Service/GetServices")
  }

  getMethodComments(serviceId : string){
    return this.httpClient.get("https://localhost:44367/api/Service/GetComments?serviceId=" + serviceId);
  }

  getMethodRatings(serviceId : string){
    return this.httpClient.get("https://localhost:44367/api/Service/GetRatings?serviceId=" + serviceId);
  }

  getMethodServicesForApproves(): Observable<any> {
    return this.httpClient.get("https://localhost:44367/api/Service/ServicesForApproves");
  }

  getMethodHasUserCommented(serviceId : string){
    return this.httpClient.get("https://localhost:44367/api/Service/HasUserCommented?serviceId=" + serviceId);
  }
  
  getMethodHasUserRated(serviceId : string){
    return this.httpClient.get("https://localhost:44367/api/Service/HasUserRated?serviceId=" + serviceId);
  }
  
  deleteService(serviceId): Observable<any>{
    return this.httpClient.delete("https://localhost:44367/api/Service/DeleteService?serviceId=" + serviceId);
  }

  postMethodApproveService(id : number) {
    return this.httpClient.post("https://localhost:44367/api/Service/ApproveService", id);
  }

  postMethodRejectService(id : number) {
    return this.httpClient.post("https://localhost:44367/api/Service/RejectService", id);
  }

  postMethodCreateRentVehicleService(rentVehicle, uploadedImage: File): Observable<any> {
    this.formData = new FormData();

    this.formData.append('name', rentVehicle.name)
    this.formData.append('contactEmail', rentVehicle.contactEmail);
    this.formData.append('description', rentVehicle.description);
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'multipart/form-data')

    return this.httpClient.post("https://localhost:44367/api/Service/PostService",this.formData, { headers: headers });
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

    return this.httpClient.put("https://localhost:44367/api/Service/PutService", this.formData, { headers: headers });
  }

}