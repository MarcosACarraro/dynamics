function Onload(executionContext) {
    formContext = executionContext.getFormContext();

    CriarContatoAjax();
    //var contato = Xrm.Page.getAttribute("primarycontactid").getValue();

    //if (contato !== null) {
    //    var contactId = contato[0].id;
    //    //console.log(contactId);
    //    //PrimeiroMetodo(contactId);
    //    Retrieve(contactId);
    //}

}


function PrimeiroMetodo(contactId)
{
  Xrm.WebApi.retrieveMultipleRecords("contact", "?$select=fullname&$filter=contactid eq 204ba0e5b9-88df-e311-b8e5-6c3be5a8b200").then(
    function success(result) {
        var fullname;

        for (var i = 0; i < result.entities.length; i++) {
            fullname = result.entities[i].fullname;
            alert(fullname);
        }
    });
}

function CriarContato() {
    var data = { "firstname": "Sample", "lastname": "Account" };

    Xrm.WebApi.createRecord("contact", data).then(
        function success(result) {
            alert("contato criado id :" + result.id);
        },
        function (error) {
            alert(error.message);
        }
    );
}

function Retrieve(contactid) {
    Xrm.WebApi.retrieveRecord("contact", contactid, "?$select=fullname").then(
        function success(result) {
            alert("contato  :" + result.fullname);
        },
        function (error) {
            alert(error.message);
        }
    );
}

function CriarContatoAjax()
{
	//shared/jquery/2.2.9/libs/jquery.js

	var contact ={};
	contact.FirstName= "Treinamento 02";
	contact.LastName="CRM Extending";
	
	var jsonObject = window.JSON.stringify(contact);
	
	$.ajax({
		type:"POST",
		contentType:"application/json;charset=uft-8",
		datatype:"json",
		url:Xrm.Page.context.getClientUrl()+ "/XRMServices/2011/OrganizationData.svc/ContactSet",
		data:jsonObject,
		beforeSend:function(XMLHttpRequest){
			XMLHttpRequest.setRequestHeader("accept","application/json")
		},
		success:function(data,textStatus,XMLHttpRequest){
			console.table(data);
		},
		error:function(XMLHttpRequest,textStatus,errorThrown){
			console.table(textStatus);
		}
	});
}


function UpdateContato(contactId)
{
	var data ={"firstname":"Marcos","lastname":"Carraro"};
	
	Xrm.WebApi.updateRecord("contact",contactId,data).then(
        function success(result) {
            Retrieve(result.id);
        },
        function (error) {
            alert(error.message);
        }
	);
}


function DeleteContato(contactId)
{
	Xrm.WebApi.deleteRecord("contact",contactId).then(
        function success(result) {
            console.table(result);
        },
        function (error) {
            alert(error.message);
        }
	);
}
