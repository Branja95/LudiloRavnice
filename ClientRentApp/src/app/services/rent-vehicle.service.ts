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
    return this.httpClient.get("https://localhost:5001/api/Services/GetService?serviceId=" + serviceId);
  }

  getMethodServices(): Observable<any> {
    return this.httpClient.get("https://localhost:5001/api/Services/GetServices")
  }

  getMethodComments(serviceId : string){
    return this.httpClient.get("https://localhost:5001/api/Services/GetComments?serviceId=" + serviceId);
  }

  getMethodRatings(serviceId : string){
    return this.httpClient.get("https://localhost:5001/api/Services/GetRatings?serviceId=" + serviceId);
  }

  getMethodServicesForApproves(): Observable<any> {
    return this.httpClient.get("https://localhost:5001/api/Services/ServicesForApproves");
  }

  getMethodHasUserCommented(serviceId : string){
    return this.httpClient.get("https://localhost:5001/api/Services/HasUserCommented?serviceId=" + serviceId);
  }
  
  getMethodHasUserRated(serviceId : string){
    return this.httpClient.get("https://localhost:5001/api/Services/HasUserRated?serviceId=" + serviceId);
  }
  
  deleteService(serviceId): Observable<any>{
    return this.httpClient.delete("https://localhost:5001/api/Services/DeleteService?serviceId=" + serviceId);
  }

  postMethodApproveService(id : number) {
    return this.httpClient.post("https://localhost:5001/api/Services/ApproveService", id);
  }

  postMethodRejectService(id : number) {
    return this.httpClient.post("https://localhost:5001/api/Services/RejectService", id);
  }

  postMethodCreateRentVehicleService(rentVehicle, uploadedImage: File): Observable<any> {
    this.formData = new FormData();

    this.formData.append('name', rentVehicle.name)
    this.formData.append('contactEmail', rentVehicle.contactEmail);
    this.formData.append('description', rentVehicle.description);
    this.formData.append('logoImage', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'multipart/form-data')

    return this.httpClient.post("https://localhost:5001/api/Services/PostService",this.formData, { headers: headers });
  }

  editService(serviceId, service, uploadedImage: File): Observable<any>{
    this.formData = new FormData();
    
    this.formData.append('id', serviceId);
    this.formData.append('name', service.Name)
    this.formData.append('emailaddress', service.EmailAddress);
    this.formData.append('description', service.Description);
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put("https://localhost:5001/api/Services/PutService?serviceId=" + serviceId, this.formData, { headers: headers });
  }

}