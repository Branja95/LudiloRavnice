import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})

export class CommentService {
  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }


  postMethodCreateComment(comment): Observable<any>{
    this.formData = new FormData();

    this.formData.append('serviceId', comment.serviceId);
    this.formData.append('text', comment.text);
    this.formData.append('value', comment.value);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post(environment.endpointBookingPostFeedback, this.formData, { headers: headers }); 
  }
}