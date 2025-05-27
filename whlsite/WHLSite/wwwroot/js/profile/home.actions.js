$(function() {

  var ssnShown = false;
  $('.whl-action-togglelast4ssn').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    ssnShown = !ssnShown;
    if (ssnShown) {
      $('.whl-show-last4ssn').hide();
      $('.whl-hide-last4ssn').show();
    } else {
      $('.whl-show-last4ssn').show();
      $('.whl-hide-last4ssn').hide();
    }
  });

  var dobShown = false;
  $('.whl-action-toggledateofbirth').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    dobShown = !dobShown;
    if (dobShown) {
      $('.whl-show-dateofbirth').hide();
      $('.whl-hide-dateofbirth').show();
    } else {
      $('.whl-show-dateofbirth').show();
      $('.whl-hide-dateofbirth').hide();
    }
  });

  $('.whl-action-viewdocument').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const i = $(this).data('docid');
    const n = $(this).data('docname');
    const a = document.createElement('a');
    a.download = '';
    a.href = '@Url.Action("GetDocument", "Profile")?docId=' + i;
    a.click();
  });

  $('.whl-action-deletedocument').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const i = $(this).data('docid');
    const n = $(this).data('docname');
    if (confirm('Are you sure you want to delete the document - ' + n + '?')) {
      $.post(deleteDocumentUrl + '?docId=' + i)
          .done(function (data) {
            alert('Deleted document - ' + n);
            const t = setTimeout(() => {
              clearTimeout(t);
              window.location.href = '@Url.Action("Index", "Profile")#DOCS';
              window.location.reload();
            }, 1000);
          })
          .fail(function (error) {
            console.error(error);
            alert('Failed to delete document - ' + n);
            window.location.reload();
          });
    }
  });

});