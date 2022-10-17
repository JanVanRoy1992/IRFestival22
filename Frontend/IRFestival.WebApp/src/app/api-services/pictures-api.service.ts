import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PicturesApiService {
  private baseUrl = environment.apiBaseUrl + 'pictures';

  constructor(private httpClient: HttpClient) { }

  getAllUrls(): Observable<string[]> {
    const headers = new HttpHeaders().set('Ocp-Apim-Subscription-Key', '1194c998769c41a1bd03ed94f9766e1a');
    return this.httpClient.get<string[]>(`${this.baseUrl}`, { 'headers': headers });
  }

  upload(file: File): Observable<never> {
    const data = new FormData();
    data.set('file', file);
    const headers = new HttpHeaders().set('Ocp-Apim-Subscription-Key', '1194c998769c41a1bd03ed94f9766e1a');

    return this.httpClient.post<never>(`${this.baseUrl}/Upload`, data, { 'headers': headers });
  }
}
