import { Injectable } from '@angular/core';
import { getFundoApplicationsWebApiV1, LoanDto } from '../../api/generated';

@Injectable({ providedIn: 'root' })
export class LoansService {
  private api = getFundoApplicationsWebApiV1();

  async getLoans() {
    try {
      const result = await this.api.getLoans();
      return result;
    } catch (err) {
      console.error('Failed to load loans', err);
      return [];
    }
  }

  async postPayment(id: string, amount: number | string) {
    try {
      const resp = await this.api.postLoansIdPayment(id, { amount });
      return resp;
    } catch (err) {
      console.error('Payment failed', err);
      throw err;
    }
  }

  async postLoan(applicantName: string, amount: number | string, currentBalance: number | string) {
    try {
      const loanDto: LoanDto = { applicantName, amount, currentBalance, status: 'Active' } as LoanDto;
      const resp = await this.api.postLoans(loanDto);
      return resp;
    } catch (err) {
      console.error('Create loan failed', err);
      throw err;
    }
  }
}
