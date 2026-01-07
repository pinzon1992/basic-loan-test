import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@angular/cdk/dialog';

@Component({
  selector: 'app-create-loan-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="create-loan-dialog">
      <h3>Create Loan</h3>
      <form #formRef="ngForm" (ngSubmit)="submit()">
        <label>
          Applicant Name:
          <input name="applicantName" [(ngModel)]="applicantName" #appName="ngModel" required />
          <div class="field-error" *ngIf="appName.invalid && appName.touched">Applicant name is required.</div>
        </label>
        <label>
          Amount:
          <input name="amount" [(ngModel)]="amount" #amt="ngModel" type="number" step="0.01" required />
          <div class="field-error" *ngIf="amt.invalid && amt.touched">Amount is required.</div>
        </label>
        <label>
          Current Balance:
          <input name="currentBalance" [(ngModel)]="currentBalance" #curBal="ngModel" type="number" step="0.01" required />
          <div class="field-error" *ngIf="curBal.invalid && curBal.touched">Current balance is required.</div>
        </label>
        <div class="actions">
          <button type="button" (click)="cancel()">Cancel</button>
          <button type="submit">Create</button>
        </div>
      </form>
    </div>
  `,
  styles: [
    `.create-loan-dialog { background: #fff; padding: 1rem; border-radius: 6px; box-shadow: 0 6px 18px rgba(0,0,0,0.12); }
     label { display:block; margin: 0.5rem 0; }
     input { width:100%; padding:0.4rem }
     .actions { display:flex; justify-content:flex-end; gap:8px; margin-top:8px }
    `,
  ],
})
export class CreateLoanDialogComponent {
  dialogRef = inject(DialogRef<null | { applicantName: string; amount: number; currentBalance: number } | undefined>);
  applicantName = '';
  amount: number | null = null;
  currentBalance: number | null = null;

  cancel() {
    this.dialogRef.close(undefined);
  }

  submit() {
    if (!this.applicantName || this.amount == null || this.currentBalance == null) return;
    this.dialogRef.close({ applicantName: this.applicantName, amount: this.amount, currentBalance: this.currentBalance });
  }
}
