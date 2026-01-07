import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoansService } from '../services/loans.service';
import { Dialog } from '@angular/cdk/dialog';
import { PaymentDialogComponent } from '../payments/payment-dialog.component';
import { firstValueFrom } from 'rxjs';
import { CreateLoanDialogComponent } from '../loans-create/create-loan-dialog.component';

interface LoanView {
  id?: string | null;
  applicantName?: string;
  amount?: number | string;
  currentBalance?: number | string;
  status?: string;
}

@Component({
  selector: 'app-loans-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loans-list.component.html',
  styleUrls: ['./loans-list.component.scss']
})
export class LoansListComponent implements OnInit {
  loans: LoanView[] = [];
  loading = false;
  error: string | null = null;

  private dialog = inject(Dialog);
  constructor(private loansService: LoansService) {}

  async ngOnInit(): Promise<void> {
    this.loading = true;
    this.error = null;
    try {
      const resp = await this.loansService.getLoans();
      // resp may already be the typed result or raw data depending on the generator
      this.loans = Array.isArray(resp) ? resp : (resp as any)?.data || [];
    } catch (e: any) {
      this.error = e?.message || 'Failed to load loans';
    } finally {
      this.loading = false;
    }
  }

  async makePayment(loan: LoanView) {
    const ref = this.dialog.open<number | undefined>(PaymentDialogComponent, { data: { loan } });
    const amount = await firstValueFrom(ref.closed);
    if (amount == null) return;
    try {
      await this.loansService.postPayment(loan.id as string, amount);
      // Refresh list
      await this.ngOnInit();
    } catch (e: any) {
      // Try to extract a useful message from the server response
      let msg = 'Payment failed';
      try {
        if (e?.response?.data) {
          msg = typeof e.response.data === 'string' ? e.response.data : JSON.stringify(e.response.data);
        } else if (e?.message) {
          msg = e.message;
        }
      } catch (ex) {
        // ignore extraction errors
      }
      alert(msg);
      // reload loans list after showing the error
      await this.ngOnInit();
    }
  }

  async openCreateLoan() {
    const ref = this.dialog.open<{ applicantName: string; amount: number; currentBalance: number } | undefined>(CreateLoanDialogComponent);
    const result = await firstValueFrom(ref.closed) as { applicantName: string; amount: number; currentBalance: number } | undefined;
    if (!result) return;
    try {
      await this.loansService.postLoan(result.applicantName, result.amount, result.currentBalance);
      await this.ngOnInit();
    } catch (e: any) {
      this.error = e?.message || 'Create loan failed';
    }
  }
}
