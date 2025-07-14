# Receiptify

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)  
[![Build Status](https://img.shields.io/github/actions/workflow/status/yourusername/receiptify/ci.yml)](https://github.com/yourusername/receiptify/actions)  

---

## Overview

**Receiptify** is a comprehensive personal finance management tool designed to simplify expense tracking by scanning, extracting, and organizing your receipts automatically. Unlike simple OCR apps, Receiptify offers a full-stack solution integrating cutting-edge **OCR with Document AI**, **structured data extraction using Groq’s LLM**, and a seamless web and mobile experience.

> Manage your spending smarter — effortlessly scan receipts, categorize expenses, track budgets, and gain actionable insights.

---

## Key Features

- 📸 **Receipt Image Upload & OCR**  
  Upload receipt images which are processed with Google Document AI for accurate text extraction.

- 🔍 **Structured Data Extraction**  
  Use Groq LLM to parse raw OCR text into meaningful fields like merchant, date, items, total amount.

- 📊 **Expense Categorization & Summaries**  
  Automatically categorize expenses and generate monthly spending reports, forecasts, and budget tracking.

- 🔐 **Secure User Authentication**  
  User sign-up/login with Google Cloud Identity Platform for smooth and secure access.

- ☁️ **Cloud-Native Architecture**  
  Backend APIs and OCR jobs deployed on Google Cloud Run with Redis caching for speed and scalability.

- 🖥️ **Full Stack UI**  
  React/Next.js frontend with a clean, responsive design optimized for desktop and mobile.

---

## Tech Stack

| Layer            | Technology                    |
|------------------|------------------------------|
| Frontend         | React, Next.js, Tailwind CSS |
| Backend          | ASP.NET Core Web API          |
| OCR Processing   | Google Document AI            |
| AI Extraction    | Groq LLM                     |
| Authentication   | Google Cloud Identity Platform, Firebase SDK |
| Database & Cache | Redis (Google Memorystore)   |
| Cloud Hosting    | Google Cloud Run, Cloud Build|

---

## How It Works

1. **User Uploads Receipt Image**  
   Receiptify frontend sends image filename to backend Cloud Run service.

2. **OCR with Document AI**  
   Backend triggers a Document AI process to extract text from the image.

3. **Field Extraction with Groq LLM**  
   Extract key structured fields like total amount, merchant, date, and itemized purchases.

4. **Save & Categorize**  
   Parsed receipt data saved in the database, expenses auto-categorized.

5. **User Dashboard**  
   Users view expense summaries, trends, and can search/filter receipts.

---

### Setup

```bash
# Clone the repo
git clone https://github.com/yourusername/receiptify.git
cd receiptify

# Backend
cd backend
dotnet restore
dotnet build

# Frontend
cd ../frontend
npm install
npm run dev
