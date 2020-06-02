import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class RatingService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getMethodGetRating(ratingId : string){
    return this.httpClient.get("https://localhost:44383/api/Rating/GetRating?id=" + ratingId);
  }
  
  postMethodCreateRating(rating):Observable<any>{
    return this.httpClient.post("https://localhost:44383/api/Rating/PostRating", rating);
  }

  putMethodEditRating(rating): Observable<any> {
    this.formData = new FormData();

    this.formData.append('id', rating.Id)
    this.formData.append('value', rating.Value)

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put("https://localhost:44383/api/Rating/PutRating", this.formData, { headers: headers });
  }

}
