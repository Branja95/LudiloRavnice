import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { environment } from '../../environments/environment'
@Injectable({
  providedIn: 'root'
})
export class RatingService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getMethodGetRating(ratingId : string){
    return this.httpClient.get(environment.endpointBookingGetRating + ratingId);
  }
  
  postMethodCreateRating(rating):Observable<any>{
    return this.httpClient.post(environment.endpointBookingCreateRating, rating);
  }

  putMethodEditRating(rating): Observable<any> {
    this.formData = new FormData();

    this.formData.append('id', rating.Id)
    this.formData.append('value', rating.Value)

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put(environment.endpointBookingEditRating, this.formData, { headers: headers });
  }

}
