import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })

export class PaymentService { private api = 'https://localhost:44337/api/UpiPayment';

  constructor(private http: HttpClient) {}

  createOrder(amount: number) {
    return this.http.post(`${this.api}/create-order`, amount);
  }
}
