$(function () {

  $('#faqs-tab-content li:first-child button').tab('show');

  $('.whl-action-add-faq').on('click', function (e) {
    e.preventDefault();
    $.get(faqConfigAddUrl)
      .done(function (response) {
        $('#whl-title-faq-action').html('Add FAQ');
        $('#hidFaqAction').val('ADD');
        $('#faqEditorModal').find('div.modal-body').html(response);
        $('#faqEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-faq').on('click', function (e) {
    e.preventDefault();
    const faqId = $(e.currentTarget).data('faqid');
    $.get(faqConfigEditUrl + '?faqId=' + faqId)
      .done(function (response) {
        $('#whl-title-faq-action').html('Edit FAQ');
        $('#hidFaqAction').val('EDIT');
        $('#faqEditorModal').find('div.modal-body').html(response);
        $('#faqEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-faq').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidFaqAction').val();
    const faqTitle = $('#Title').val();
    const url = a === 'EDIT' ? faqConfigEditUrl : faqConfigAddUrl;
    const data = {
      FaqId: $('#FaqId').val(),
      CategoryName: $('#CategoryName').val(),
      Title: $('#Title').val(),
      Text: $('#Text').val(),
      Url: $('#Url').val(),
      Url1: $('#Url1').val(),
      Url2: $('#Url2').val(),
      Url3: $('#Url3').val(),
      Url4: $('#Url4').val(),
      Url5: $('#Url5').val(),
      Url6: $('#Url6').val(),
      Url7: $('#Url7').val(),
      Url8: $('#Url8').val(),
      Url9: $('#Url9').val(),
      DisplayOrder: $('#DisplayOrder').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#faqEditorModal').find('div.modal-body').html('');
        $('#faqEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'FAQ ' + faqTitle + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save faq information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-faq').on('click', function (e) {
    e.preventDefault();
    const faqId = $(e.currentTarget).data('faqid');
    const faqTitle = $(e.currentTarget).data('faqtitle');
    if (confirm('Are you sure you want to delete FAQ - ' + faqTitle + '?')) {
      $.post(faqConfigDeleteUrl + '?faqId=' + faqId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'FAQ ' + faqTitle + ' deleted, refreshing page.');
        });
    }
  });

});