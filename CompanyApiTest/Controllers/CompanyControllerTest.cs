﻿using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Controllers;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            //when
            var response = await httpclient.PostAsync("/companies", postBody);
            //then
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async void Should_return_conflict_when_company_already_exsited()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            //when
            await httpclient.PostAsync("/companies", postBody);
            var response = await httpclient.PostAsync("/companies", postBody);
            //then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_obtain_all_exsiting_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var companies = new List<Company>
            {
                new Company(name: "Katy"),
                new Company(name: "Toms"),
                new Company(name: "Andy"),
            };
            foreach (var company in companies)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpclient.PostAsync("/companies", postBody);
            }

            //when
            var response = await httpclient.GetAsync("/companies");
            //then
            var responseBody = await response.Content.ReadAsStringAsync();
            var companinesObtained = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, companinesObtained.Count);
        }

        [Fact]
        public async void Should_obtain_an_exsiting_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var companies = new List<Company>
            {
                new Company(name: "Katy"),
                new Company(name: "Toms"),
                new Company(name: "Andy"),
            };
            foreach (var company in companies)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpclient.PostAsync("/companies", postBody);
            }

            var response = await httpclient.GetAsync("/companies");
            var responseBody = await response.Content.ReadAsStringAsync();
            var companinesObtained = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            var companyId = companinesObtained[0].CompanyID;
            //when
            var responseCompany = await httpclient.GetAsync($"/companies/{companyId}");
            //then
            var responseCompanyBody = await responseCompany.Content.ReadAsStringAsync();
            var companyGot = JsonConvert.DeserializeObject<Company>(responseCompanyBody);
            Assert.Equal(HttpStatusCode.OK, responseCompany.StatusCode);
            Assert.Equal("Katy", companyGot.Name);
        }

        [Fact]
        public async void Should_obtain_page_index_of_the_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var companies = new List<Company>
            {
                new Company(name: "Katy"),
                new Company(name: "Toms"),
                new Company(name: "Andy"),
                new Company(name: "SLB"),
                new Company(name: "BGC"),
            };
            foreach (var company in companies)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpclient.PostAsync("/companies", postBody);
            }

            //when
            var responseCompany = await httpclient.GetAsync("/companies?pageSize=3&pageIndex=2");
            //then
            var responseCompanyBody = await responseCompany.Content.ReadAsStringAsync();
            var companyGot = JsonConvert.DeserializeObject<List<Company>>(responseCompanyBody);
            Assert.Equal(HttpStatusCode.OK, responseCompany.StatusCode);
            Assert.Equal(2, companyGot.Count);
            Assert.Equal("SLB", companyGot[0].Name);
            Assert.Equal("BGC", companyGot[1].Name);
        }

        [Fact]
        public async void Should_update_the_name_of_the_exsiting_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var companies = new List<Company>
            {
                new Company(name: "Katy"),
                new Company(name: "Toms"),
                new Company(name: "Andy"),
                new Company(name: "SLB"),
                new Company(name: "BGC"),
            };
            foreach (var company in companies)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpclient.PostAsync("/companies", postBody);
            }

            var response = await httpclient.GetAsync("/companies");
            var responseBody = await response.Content.ReadAsStringAsync();
            var companinesObtained = JsonConvert.DeserializeObject<List<Company>>(responseBody);

            var companyId = companinesObtained[4].CompanyID;
            companinesObtained[4].Name = "ABCD";
            var companiesJson = JsonConvert.SerializeObject(companinesObtained[4]);
            var postCompaniesBody = new StringContent(companiesJson, Encoding.UTF8, "application/json");
            //when
            var responseCompanies = await httpclient.PutAsync($"/companies/{companyId}", postCompaniesBody);
            //then
            var responseBodyCompanies = await responseCompanies.Content.ReadAsStringAsync();
            var companiesNew = JsonConvert.DeserializeObject<List<Company>>(responseBodyCompanies);
            Assert.Equal(HttpStatusCode.OK, responseCompanies.StatusCode);
            Assert.Equal("ABCD", companiesNew[4].Name);
        }

        [Fact]
        public async void Should_be_able_to_add_an_employee_to_a_specific_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var companies = new List<Company>
             {
                 new Company(name: "PEPSI"),
                 new Company(name: "COLA"),
                 new Company(name: "FANTA"),
                 new Company(name: "SLB"),
                 new Company(name: "BGC"),
             };
            foreach (var company in companies)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpclient.PostAsync("/companies", postBody);
            }

            var response = await httpclient.GetAsync("/companies");
            var responseBody = await response.Content.ReadAsStringAsync();
            var companinesObtained = JsonConvert.DeserializeObject<List<Company>>(responseBody);

            var companyId = companinesObtained[3].CompanyID;
            var employee = new Employee(name: "Katy", salary: "10w");
            var employeeJson = JsonConvert.SerializeObject(employee);
            var postBodyEmployee = new StringContent(employeeJson, Encoding.UTF8, "application/json");
            //when
            var responseCompanies = await httpclient.PostAsync($"/companies/{companyId}/employees", postBodyEmployee);
            //then
            var responseBodyCompanies = await responseCompanies.Content.ReadAsStringAsync();
            var companiesNew = JsonConvert.DeserializeObject<List<Company>>(responseBodyCompanies);
            Assert.Equal(HttpStatusCode.OK, responseCompanies.StatusCode);
            Assert.Equal("Katy", companiesNew[3].Employees[0].Name);
        }
    }
}
