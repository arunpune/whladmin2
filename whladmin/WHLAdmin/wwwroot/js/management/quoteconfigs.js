$(function () {

  $('.whl-action-add-quote').on('click', function (e) {
    e.preventDefault();
    $.get(quoteConfigAddUrl)
      .done(function (response) {
        $('#whl-title-quote-action').html('Add Quote');
        $('#hidQuoteAction').val('ADD');
        $('#quoteEditorModal').find('div.modal-body').html(response);
        $('#quoteEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-quote').on('click', function (e) {
    e.preventDefault();
    var quoteId = $(e.currentTarget).data('quoteid');
    $.get(quoteConfigEditUrl + '?quoteId=' + quoteId)
      .done(function (response) {
        $('#whl-title-quote-action').html('Edit Quote');
        $('#hidQuoteAction').val('EDIT');
        $('#quoteEditorModal').find('div.modal-body').html(response);
        $('#quoteEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-quote').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    var a = $('#hidQuoteAction').val();
    var quoteText = $('#Text').val();
    var url = a === 'EDIT' ? quoteConfigEditUrl : quoteConfigAddUrl;
    var data = {
      QuoteId: $('#QuoteId').val(),
      Text: $('#Text').val(),
      DisplayOnHomePageInd: $('#DisplayOnHomePageInd').prop('checked'),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#quoteEditorModal').find('div.modal-body').html('');
        $('#quoteEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Quote ' + quoteText + ' saved, refreshing page.');
      })
      .fail(function (e) {
        var message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save quote information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-quote').on('click', function (e) {
    e.preventDefault();
    var quoteId = $(e.currentTarget).data('quoteid');
    var quoteText = $(e.currentTarget).data('quotetext');
    if (confirm('Are you sure you want to delete quote - ' + quoteText + '?')) {
      $.post(quoteConfigDeleteUrl + '?quoteId=' + quoteId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Quote ' + quoteText + ' deleted, refreshing page.');
        });
    }
  });

});